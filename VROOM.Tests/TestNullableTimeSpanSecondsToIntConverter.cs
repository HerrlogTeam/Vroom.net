﻿using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VROOM.Converters;

namespace VROOM.Tests
{
    [TestClass]
    public class TestNullableTimeSpanSecondsToIntConverter
    {
        private static readonly TimeSpan?[] TestValues = new TimeSpan?[]
        {
            new TimeSpan(10, 10, 10),
            new TimeSpan(0, 0, 10),
            new TimeSpan(100, 100, 100),
            null
        };

        [TestMethod]
        public void CanSerialize()
        {
            NullableTimeSpanSecondsToIntConverter converter = new NullableTimeSpanSecondsToIntConverter();

            foreach (var value in TestValues)
            {
                using MemoryStream stream = new MemoryStream();
                using Utf8JsonWriter writer = new Utf8JsonWriter(stream);

                converter.Write(writer, value, new JsonSerializerOptions());

                writer.Flush();
                stream.Position = 0;

                using TextReader reader = new StreamReader(stream);
                string result = reader.ReadToEnd();

                result.Should().Be(value == null ? "null" : ((int) Math.Round(value.Value.TotalSeconds)).ToString());
            }
        }

        [TestMethod]
        public void CanDeserialize()
        {
            NullableTimeSpanSecondsToIntConverter converter = new NullableTimeSpanSecondsToIntConverter();
            foreach (var value in TestValues)
            {
                Utf8JsonReader reader =
                    new Utf8JsonReader(Encoding.UTF8.GetBytes(value == null ? "null" : ((int) Math.Round(value.Value.TotalSeconds)).ToString()));
                reader.Read();
                var result = converter.Read(ref reader, typeof(TimeSpan?), new JsonSerializerOptions());

                result.Should().Be(value);
            }
        }
    }
}