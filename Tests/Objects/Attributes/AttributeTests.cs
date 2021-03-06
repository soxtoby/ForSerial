﻿using System;
using EasyAssertions;
using ForSerial.Objects;
using NSubstitute;

namespace ForSerial.Tests.Objects
{
    public abstract class AttributeTests<T> where T : PropertyDefinitionAttribute
    {
        protected abstract T CreateAttribute();

        protected void TestNameOverride(string scenario, string baseName, string expectedName)
        {
            TestAttribute(scenario,
                innerDefinition => innerDefinition.SerializedName.Returns(baseName),
                sut => sut.SerializedName.ShouldBe(expectedName));
        }

        protected void TestCanGetOverride(string scenario, bool baseCanGet, bool expectedCanGet)
        {
            TestAttribute(scenario,
                innerDefinition => innerDefinition.CanGet.Returns(baseCanGet),
                sut => sut.CanGet.ShouldBe(expectedCanGet));
        }

        protected void TestCanSetOverride(string scenario, bool baseCanSet, bool expectedCanSet)
        {
            TestAttribute(scenario,
                innerDefinition => innerDefinition.CanSet.Returns(baseCanSet),
                sut => sut.CanSet.ShouldBe(expectedCanSet));
        }

        protected void TestReadOverride(string scenario, Action<T> assertion)
        {
            TestAttribute(scenario, definition => { }, sut =>
                {
                    sut.Read(null, null, null);
                    assertion(sut);
                });
        }

        protected void TestCanCreateValueOverride(string scenario, bool baseCanCreateValue, bool expected)
        {
            TestAttribute(scenario,
                innerDefinition => innerDefinition.CanCreateValue(null).Returns(baseCanCreateValue),
                sut => sut.CanCreateValue(null).ShouldBe(expected));
        }

        protected void TestCreateSequenceOverride(string scenario, ObjectContainer baseSequence, ObjectContainer expectedSequence)
        {
            TestAttribute(scenario,
                innerDefinition => innerDefinition.CreateSequence().Returns(baseSequence),
                sut => sut.CreateSequence().ShouldBe(expectedSequence));
        }

        protected void TestCreateDefaultStructureOverride(string scenario, ObjectContainer baseStructure, ObjectContainer expectedStructure)
        {
            TestAttribute(scenario,
                innerDefinition => innerDefinition.CreateStructure().Returns(baseStructure),
                sut => sut.CreateStructure().ShouldBe(expectedStructure));
        }

        protected void TestCreateTypedStructureOverride(string scenario, ObjectContainer baseStructure, ObjectContainer expectedStructure)
        {
            const string typeIdentifier = "foo";
            TestAttribute(scenario,
                innerDefinition => innerDefinition.CreateStructure(typeIdentifier).Returns(baseStructure),
                sut => sut.CreateStructure(typeIdentifier).ShouldBe(expectedStructure));
        }

        protected void TestCreateValueOverride(string scenario, ObjectOutput baseValue, ObjectOutput expectedValue)
        {
            TestAttribute(scenario,
                innerDefinition => innerDefinition.CreateValue(null).Returns(baseValue),
                sut => sut.CreateValue(null).ShouldBe(expectedValue));
        }

        protected void TestMatchesPropertyFilterOverride(string scenario, bool baseMatches, bool expected)
        {
            ObjectParsingOptions options = new ObjectParsingOptions();
            TestAttribute(scenario,
                innerDefinition => innerDefinition.MatchesPropertyFilter(options).Returns(baseMatches),
                sut => sut.MatchesPropertyFilter(options).ShouldBe(expected));
        }

        private void TestAttribute(string scenario, Action<PropertyDefinition> setupInnerDefinition, Action<T> assertion)
        {
            T sut = CreateAttribute();
            sut.InnerDefinition = Substitute.For<PropertyDefinition>();
            setupInnerDefinition(sut.InnerDefinition);
            using (SerializationScenario.Override(scenario))
            {
                assertion(sut);
            }
        }
    }
}