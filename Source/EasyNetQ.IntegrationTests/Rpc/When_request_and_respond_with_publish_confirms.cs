using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace EasyNetQ.IntegrationTests.Rpc
{
    [Collection("RabbitMQ")]
    public class When_request_and_respond_with_publish_confirms : IDisposable
    {
        public When_request_and_respond_with_publish_confirms(RabbitMQFixture fixture)
        {
            bus = RabbitHutch.CreateBus($"host={fixture.Host};prefetchCount=1;publisherConfirms=True;timeout=-1");
        }

        public void Dispose()
        {
            bus.Dispose();
        }

        private readonly IBus bus;

        [Fact]
        public async Task Should_receive_exception()
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            using (
                await bus.Rpc.RespondAsync<Request, Response>(
                    x => Task.FromException<Response>(new RequestFailedException()), cts.Token
                )
            )
            {
                await Assert.ThrowsAsync<EasyNetQResponderException>(
                    () => bus.Rpc.RequestAsync<Request, Response>(new Request(42), cts.Token)
                );
            }
        }

        [Fact]
        public async Task Should_receive_response()
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            using (await bus.Rpc.RespondAsync<Request, Response>(x => new Response(x.Id), cts.Token))
            {
                var response = await bus.Rpc.RequestAsync<Request, Response>(new Request(42), cts.Token);
                response.Should().Be(new Response(42));
            }
        }
    }
}
