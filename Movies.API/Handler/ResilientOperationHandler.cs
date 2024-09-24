using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using System.Threading.Tasks;
using System;

namespace Movies.API.Handler
{
    /// <summary>
    /// This setup allows you to handle all failures from a central point and ensures that downstream services are protected from cascading failures.
    /// </summary>
    public class ResilientOperationHandler
    {
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;

        public ResilientOperationHandler()
        {
            // Define the retry policy
            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: 3, // Number of retry attempts
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // Exponential backoff
                    onRetry: (exception, timespan, retryCount, context) =>
                    {
                        // Log each retry attempt
                        Console.WriteLine($"Retry {retryCount} due to {exception.Message}. Waiting {timespan} before next retry.");
                    });

            // Define the circuit breaker policy
            _circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 2, // Number of exceptions before breaking the circuit
                    durationOfBreak: TimeSpan.FromMinutes(1), // How long the circuit remains open
                    onBreak: (exception, timespan) =>
                    {
                        // Log when the circuit is broken
                        Console.WriteLine($"Circuit broken due to {exception.Message}. Breaking for {timespan}.");
                    },
                    onReset: () =>
                    {
                        // Log when the circuit resets
                        Console.WriteLine("Circuit reset.");
                    },
                    onHalfOpen: () =>
                    {
                        // Log when the circuit is half-open
                        Console.WriteLine("Circuit is half-open. Testing if operations can proceed.");
                    });
        }

        /// <summary>
        /// The ExecuteAsync methods in ResilientOperationHandler ensure that any operation you pass to it will first be subjected to the retry policy, and if it continues to fail, the circuit breaker will prevent further attempts.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="operation"></param>
        /// <returns></returns>
        public async Task<T> ExecuteAsync<T>(Func<Task<T>> operation)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                return await _circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    return await operation();
                });
            });
        }

        /// <summary>
        /// The ExecuteAsync methods in ResilientOperationHandler ensure that any operation you pass to it will first be subjected to the retry policy, and if it continues to fail, the circuit breaker will prevent further attempts.
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        public async Task ExecuteAsync(Func<Task> operation)
        {
            await _retryPolicy.ExecuteAsync(async () =>
            {
                await _circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    await operation();
                });
            });
        }
    }
}
