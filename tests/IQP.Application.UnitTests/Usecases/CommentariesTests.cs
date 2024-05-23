namespace IQP.Application.UnitTests.Usecases;

[TestFixture]
[Category("UnitTests")]
public class CommentariesTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Creating_Reply_To_NonExistingCommentary_Throws_NotFound()
    {
        Assert.Pass();
    }

    [Test]
    public void Replying_To_Reply_Propagates_Commentary_to_Root()
    {
        Assert.Pass();
    }
}
