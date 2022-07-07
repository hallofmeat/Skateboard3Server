using System.Buffers;
using System.IO;
using FluentAssertions;
using Skateboard3Server.Blaze.Serializer;
using Xunit;

namespace Skateboard3Server.Blaze.Tests.Serializer;

public class TdfHelperFacts
{
    [Fact]
    public void ParseTag_parsed_correctly()
    {
        var data = new byte[] { 0xBA, 0x1B, 0x65 }; //NAME

        var sequence = new ReadOnlySequence<byte>(data);
        var reader = new SequenceReader<byte>(sequence);

        //Act
        var result = TdfHelper.ParseTag(ref reader);

        //Assert
        result.Should().Be("NAME");
    }

    [Fact]
    public void ParseLength_small_value_parsed_correctly()
    {
        var data = new byte[] { 0xA }; //10

        var sequence = new ReadOnlySequence<byte>(data);
        var reader = new SequenceReader<byte>(sequence);

        //Act
        var result = TdfHelper.ParseLength(ref reader);

        //Assert
        result.Should().Be(10);
    }

    [Fact]
    public void ParseLength_large_value_parsed_correctly()
    {
        var data = new byte[] { 0x87, 0x68 }; //1,000

        var sequence = new ReadOnlySequence<byte>(data);
        var reader = new SequenceReader<byte>(sequence);

        //Act
        var result = TdfHelper.ParseLength(ref reader);

        //Assert
        result.Should().Be(1000);
    }

    [Fact]
    public void ParseTypeAndLength_large_length_parsed_correctly()
    {
        var data = new byte[] { 0xBF, 0x87, 0x68 }; //Blob 1,000

        var sequence = new ReadOnlySequence<byte>(data);
        var reader = new SequenceReader<byte>(sequence);

        //Act
        var result = TdfHelper.ParseTypeAndLength(ref reader);

        //Assert
        result.Type.Should().Be(TdfType.Blob);
        result.Length.Should().Be(1000);
    }

    [Fact]
    public void ParseTypeAndLength_small_length_parsed_correctly()
    {
        var data = new byte[] { 0x1A }; //String 10

        var sequence = new ReadOnlySequence<byte>(data);
        var reader = new SequenceReader<byte>(sequence);

        //Act
        var result = TdfHelper.ParseTypeAndLength(ref reader);

        //Assert
        result.Type.Should().Be(TdfType.String);
        result.Length.Should().Be(10);
    }

    [Fact]
    public void WriteTag_created_correctly()
    {
        var resultStream = new MemoryStream();

        //Act
        TdfHelper.WriteTag(resultStream, "NAME");

        //Assert
        var valid = new byte[] { 0xBA, 0x1B, 0x65 }; //NAME
        var result = resultStream.ToArray();
        result.Should().BeEquivalentTo(valid);
    }

    [Fact]
    public void WriteLength_large_value_correctly()
    {
        var resultStream = new MemoryStream();

        //Act
        TdfHelper.WriteLength(resultStream, 1000);

        //Assert
        var valid = new byte[] { 0x87, 0x68 }; //1000
        var result = resultStream.ToArray();
        result.Should().BeEquivalentTo(valid);
    }

    [Fact]
    public void WriteLength_small_value_correctly()
    {
        var resultStream = new MemoryStream();

        //Act
        TdfHelper.WriteLength(resultStream, 10);

        //Assert
        var valid = new byte[] { 0xA }; //10
        var result = resultStream.ToArray();
        result.Should().BeEquivalentTo(valid);
    }

    [Fact]
    public void WriteType_correctly()
    {
        var resultStream = new MemoryStream();

        //Act
        TdfHelper.WriteType(resultStream, TdfType.Blob);

        //Assert
        var valid = new byte[] { 0xBF }; //Blob
        var result = resultStream.ToArray();
        result.Should().BeEquivalentTo(valid);
    }

    [Fact]
    public void WriteTypeAndLength_large_value_correctly()
    {
        var resultStream = new MemoryStream();

        //Act
        TdfHelper.WriteTypeAndLength(resultStream, TdfType.Blob, 1000);

        //Assert
        var valid = new byte[] { 0xBF, 0x87, 0x68 }; //Blob 1000
        var result = resultStream.ToArray();
        result.Should().BeEquivalentTo(valid);
    }

    [Fact]
    public void WriteTypeAndLength_small_value_correctly()
    {
        var resultStream = new MemoryStream();

        //Act
        TdfHelper.WriteTypeAndLength(resultStream, TdfType.String, 10);

        //Assert
        var valid = new byte[] { 0x1A }; //String 10
        var result = resultStream.ToArray();
        result.Should().BeEquivalentTo(valid);
    }


}