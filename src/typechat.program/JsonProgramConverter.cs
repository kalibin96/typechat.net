﻿// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.TypeChat;

public class JsonProgramConvertor : JsonConverter<Program>
{
    static ProgramParser DefaultParser = new ProgramParser();

    public override Program? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        JsonDocument doc = JsonDocument.ParseValue(ref reader);
        return DefaultParser.Parse(doc);
    }

    public override void Write(Utf8JsonWriter writer, Program value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}