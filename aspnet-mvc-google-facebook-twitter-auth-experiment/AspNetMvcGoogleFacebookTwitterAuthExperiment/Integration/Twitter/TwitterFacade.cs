using System.Collections.Specialized;
using OAuth2.Client.Impl;
using OAuth2.Configuration;
using OAuth2.Infrastructure;
using RestSharp;
using RestSharp.Authenticators;

namespace AspNetMvcGoogleFacebookTwitterAuthExperiment.Integration.Twitter
{
    public class TwitterFacade
    {
        private readonly string _twitterConsumerKey;
        private readonly string _twitterConsumerSecret;
        private readonly string _twitterCallbackUrl;

        public TwitterFacade(
            string twitterConsumerKey, 
            string twitterConsumerSecret,
            string twitterCallbackUrl)
        {
            _twitterConsumerKey = twitterConsumerKey;
            _twitterConsumerSecret = twitterConsumerSecret;
            _twitterCallbackUrl = twitterCallbackUrl;
        }

        public string GetAuthenticationUrl()
        {
            var twitterClient = MakeTwitterClient();
            return twitterClient.GetLoginLinkUri();
        }

        public TwitterUserInfo GetUserInfo(string oauthToken, string oauthVerifier)
        {
            var parameters = new NameValueCollection
                {
                    {"oauth_token", oauthToken},
                    {"oauth_verifier", oauthVerifier}
                };

            var twitterClient = MakeTwitterClient();
            var userInfo = twitterClient.GetUserInfo(parameters);
            if (userInfo == null)
            {
                return null;
            }

            return new TwitterUserInfo
                {
                    UserId = userInfo.Id,
                    AccessToken = twitterClient.AccessToken,
                    AccessTokenSecret = twitterClient.AccessTokenSecret
                };
        }

        public CustomTwitterUserInfo GetCustomUserInfo(string accessToken, string accessTokenSecret)
        {
            var client = new RestClient("https://api.twitter.com/")
                {
                    Authenticator = OAuth1Authenticator.ForProtectedResource(
                        _twitterConsumerKey,
                        _twitterConsumerSecret,
                        accessToken,
                        accessTokenSecret)
                };

            var request = new RestRequest("/1.1/account/verify_credentials.json", Method.GET);
            var response = client.Execute<CustomTwitterUserInfo>(request);
            return response.Data;
        }

        private TwitterClient MakeTwitterClient()
        {
            var twitterClient = new TwitterClient(
                new RequestFactory(),
                new RuntimeClientConfiguration
                    {
                        ClientId = _twitterConsumerKey,
                        ClientSecret = _twitterConsumerSecret,
                        IsEnabled = true,
                        RedirectUri = UriUtility.ToAbsolute(_twitterCallbackUrl)
                    });

            return twitterClient;
        }
    }
}