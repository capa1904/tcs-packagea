using NUnit.Framework;

namespace PackageA.Test.Unit
{

    [TestFixture]
    public class ProgrammTest
    {
        [Test]
        public void GetName_NameIsNotEmpty()
        {
            var subject = new ClassOfPackageA();
            Assert.IsNotNullOrEmpty(subject.GetName());
        }
    }
}
