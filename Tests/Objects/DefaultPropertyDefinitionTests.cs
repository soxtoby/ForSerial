using EasyAssertions;
using ForSerial.Objects;
using NSubstitute;
using NUnit.Framework;

namespace ForSerial.Tests.Objects
{
    [TestFixture]
    public class DefaultPropertyDefinitionTests
    {
        [Test]
        public void MatchesFilter_MemberIsPublicGetSet_MatchesPublicGetSet()
        {
            AccessibilityMatches(MemberAccessibility.PublicGetSet, MemberAccessibility.PublicGetSet, true);
        }

        [Test]
        public void MatchesFilter_MemberIsPublicGetSet_MatchesPublicGet()
        {
            AccessibilityMatches(MemberAccessibility.PublicGetSet, MemberAccessibility.PublicGet, true);
        }

        [Test]
        public void MatchesFilter_MemberIsPublicGetSet_DoesNotMatchPrivate()
        {
            AccessibilityMatches(MemberAccessibility.PublicGetSet, MemberAccessibility.Private, false);
        }

        [Test]
        public void MatchesFilter_MembersIsPublicGet_DoesNotMatchPublicGetSet()
        {
            AccessibilityMatches(MemberAccessibility.PublicGet, MemberAccessibility.PublicGetSet, false);
        }

        [Test]
        public void MatchesFilter_MemberIsPublicGet_MatchesPublicGet()
        {
            AccessibilityMatches(MemberAccessibility.PublicGet, MemberAccessibility.PublicGet, true);
        }

        [Test]
        public void MatchesFilter_MemberIsPublicGet_DoesNotMatchPrivate()
        {
            AccessibilityMatches(MemberAccessibility.PublicGet, MemberAccessibility.Private, false);
        }

        [Test]
        public void MatchesFilter_MemberIsPrivate_DoesNotMatchPublicGetSet()
        {
            AccessibilityMatches(MemberAccessibility.Private, MemberAccessibility.PublicGetSet, false);
        }

        [Test]
        public void MatchesFilter_MemberIsPrivate_DoesNotMatchPublicGet()
        {
            AccessibilityMatches(MemberAccessibility.Private, MemberAccessibility.PublicGet, false);
        }

        [Test]
        public void MatchesFilter_MemberIsPrivate_MatchesPrivate()
        {
            AccessibilityMatches(MemberAccessibility.Private, MemberAccessibility.Private, true);
        }

        private static void AccessibilityMatches(MemberAccessibility memberAccessibility, MemberAccessibility requiredAccessibility, bool expected)
        {
            DefaultPropertyDefinition sut = CreatePropertyDefinition(memberAccessibility, MemberType.Property);
            sut.MatchesPropertyFilter(requiredAccessibility, MemberType.Property)
                .ShouldBe(expected);
        }

        [Test]
        public void MatchesFilter_MemberIsProperty_MatchesProperty()
        {
            TypeMatches(MemberType.Property, MemberType.Property, true);
        }

        [Test]
        public void MatchesFilter_MemberIsProperty_DoesNotMatchField()
        {
            TypeMatches(MemberType.Property, MemberType.Field, false);
        }

        [Test]
        public void MatchesFilter_MemberIsProperty_MatchesEither()
        {
            TypeMatches(MemberType.Property, MemberType.Either, true);
        }

        [Test]
        public void MatchesFilter_MemberIsField_DoesNotMatchProperty()
        {
            TypeMatches(MemberType.Field, MemberType.Property, false);
        }

        [Test]
        public void MatchesFilter_MemberIsField_MatchesField()
        {
            TypeMatches(MemberType.Field, MemberType.Field, true);
        }

        [Test]
        public void MatchesFilter_MemberIsField_MatchesEither()
        {
            TypeMatches(MemberType.Field, MemberType.Either, true);
        }

        private static void TypeMatches(MemberType memberType, MemberType requiredType, bool expected)
        {
            DefaultPropertyDefinition sut = CreatePropertyDefinition(MemberAccessibility.PublicGetSet, memberType);
            sut.MatchesPropertyFilter(MemberAccessibility.PublicGetSet, requiredType)
                .ShouldBe(expected);
        }

        private static DefaultPropertyDefinition CreatePropertyDefinition(MemberAccessibility memberAccessibility, MemberType memberType)
        {
            return new DefaultPropertyDefinition(Substitute.For<TypeDefinition>(typeof(object)), string.Empty, null, null, string.Empty, memberAccessibility, memberType);
        }
    }
}