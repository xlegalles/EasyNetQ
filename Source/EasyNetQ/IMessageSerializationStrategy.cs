using System.Buffers;

namespace EasyNetQ;

/// <summary>
///     The message serialization strategy
/// </summary>
public interface IMessageSerializationStrategy
{
    /// <summary>
    ///     Serializes the message
    /// </summary>
    /// <param name="message">The message</param>
    /// <returns></returns>
    SerializedMessage SerializeMessage<TMessage>(in TMessage message) where TMessage : IMessage;

    /// <summary>
    ///     Deserializes the message
    /// </summary>
    /// <param name="properties">The properties</param>
    /// <param name="body">The body</param>
    /// <returns></returns>
    IMessage DeserializeMessage(in MessageProperties properties, in ReadOnlyMemory<byte> body);
}

/// <summary>
///     Represents a serialized message
/// </summary>
public readonly struct SerializedMessage : IDisposable
{
    private readonly IDisposable owner;

    /// <summary>
    ///     Creates SerializedMessage
    /// </summary>s
    public SerializedMessage(in MessageProperties properties, IMemoryOwner<byte> body)
    {
        Properties = properties;
        Body = body.Memory;
        owner = body;
    }

    /// <summary>
    ///     Message properties
    /// </summary>
    public MessageProperties Properties { get; }

    /// <summary>
    ///     Message body
    /// </summary>
    public ReadOnlyMemory<byte> Body { get; }

    /// <inheritdoc />
    public void Dispose() => owner?.Dispose();
}
