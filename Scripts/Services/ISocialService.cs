namespace Chocolate.Services
{
    public interface ISocialService
    {
        public void TweetWithScreenShotAsync(string tweetText, string tweetUrl);
    }
}
