﻿using System;
using System.Collections.Generic;
using EntityFrameworkInheritanceExperiment.DAL;
using EntityFrameworkInheritanceExperiment.DTO;
using EntityFrameworkInheritanceExperiment.Service.Configuration;
using EntityFrameworkInheritanceExperiment.Service.TransactionScripts;
using Ninject;

namespace EntityFrameworkInheritanceExperiment.Service
{
    [Service]
    public class AuthenticationService
    {
        [Inject]
        [Named("ConnectionStringName")]
        public string ConnectionStringName { private get; set; }

        [Inject] public ResetTransactionScript ResetTransactionScript { private get; set; }
        [Inject] public SignUpWithEmailAndPasswordTransactionScript SignUpWithEmailAndPasswordTransactionScript { private get; set; }
        [Inject] public SignInWithEmailAndPasswordTransactionScript SignInWithEmailAndPasswordTransactionScript { private get; set; }
        [Inject] public AuthenticateWithGoogleTransactionScript AuthenticateWithGoogleTransactionScript { private get; set; }
        [Inject] public AuthenticateWithFacebookTransactionScript AuthenticateWithFacebookTransactionScript { private get; set; }
        [Inject] public AuthenticateWithTwitterTransactionScript AuthenticateWithTwitterTransactionScript { private get; set; }
        [Inject] public AddEmailTransactionScript AddEmailTransactionScript { private get; set; }
        [Inject] public AddGoogleUserIdTransactionScript AddGoogleTransactionScript { private get; set; }
        [Inject] public AddFacebookUserIdTransactionScript AddFacebookTransactionScript { private get; set; }
        [Inject] public AddTwitterDisplayNameTransactionScript AddTwitterTransactionScript { private get; set; }
        [Inject] public DeleteAuthenticationMethodTransactionScript DeleteAuthenticationMethodTransactionScript { private get; set; }
        [Inject] public GetUserCountTransactionScript GetUserCountTransactionScript { private get; set; }
        [Inject] public GetUserTransactionScript GetUserTransactionScript { private get; set; }
        [Inject] public GetAllUsersTransactionScript GetAllUsersTransactionScript { private get; set; }

        public void Reset()
        {
            ResetTransactionScript.Reset();
        }

        public UserDTO SignUpWithEmailAndPassword(string email, string password)
        {
            return Run(context => SignUpWithEmailAndPasswordTransactionScript
                .SignUpWithEmailAndPassword(
                    context, 
                    email, 
                    password));
        }

        public UserDTO SignInWithEmailAndPassword(string email, string password)
        {
            return Run(context => SignInWithEmailAndPasswordTransactionScript
                .SignInWithEmailAndPassword(
                    context, 
                    email, 
                    password));
        }

        public UserDTO AuthenticateWithGoogle(string googleUserId, string email)
        {
            return Run(context => AuthenticateWithGoogleTransactionScript
                .AuthenticateWithGoogle(
                    context,
                    googleUserId, 
                    email));
        }

        public UserDTO AuthenticateWithFacebook(string facebookUserId, string email)
        {
            return Run(context => AuthenticateWithFacebookTransactionScript
                .AuthenticateWithFacebook(
                    context,
                    facebookUserId,
                    email));
        }

        public UserDTO AuthenticateWithTwitter(string twitterUserId, string twitterDisplayName)
        {
            return Run(context => AuthenticateWithTwitterTransactionScript
                .AuthenticateWithTwitter(
                    context, 
                    twitterUserId,
                    twitterDisplayName));
        }

        public UserDTO AddEmailAndPassword(int userId, string email, string password)
        {
            return Run(context => AddEmailTransactionScript
                .AddEmail(
                    context, 
                    userId, 
                    email));
        }

        public UserDTO AddGoogle(int userId, string googleUserId, string email)
        {
            return Run(context => AddGoogleTransactionScript
                .AddGoogleUserId(
                    context, 
                    userId, 
                    googleUserId, 
                    email));
        }

        public UserDTO AddFacebook(int userId, string facebookUserId, string email)
        {
            return Run(context => AddFacebookTransactionScript
                .AddFacebookUserId(
                    context, 
                    userId, 
                    facebookUserId, 
                    email));
        }

        public UserDTO AddTwitter(int userId, string twitterUserId, string twitterDisplayName)
        {
            return Run(context => AddTwitterTransactionScript
                .AddTwitterDisplayName(
                    context, 
                    userId, 
                    twitterUserId,
                    twitterDisplayName));
        }

        public UserDTO DeleteAuthenticationMethod(int userId, int authenticationMethodId)
        {
            return Run(context => DeleteAuthenticationMethodTransactionScript
                .DeleteAuthenticationMethod(
                    context, 
                    userId, 
                    authenticationMethodId));
        }

        public IList<UserDTO> GetAllUsers()
        {
            return Run(context => GetAllUsersTransactionScript.GetAllUsers(context));
        }

        public int GetUserCount()
        {
            return Run(context => GetUserCountTransactionScript.GetUserCount(context));
        }

        public UserDTO GetUser(int userId)
        {
            return Run(context => GetUserTransactionScript
                .GetUser(
                    context, 
                    userId));
        }

        private T Run<T>(Func<UserContext, T> func)
        {
            using (var context = new UserContext(ConnectionStringName))
            {
                return func(context);
            }
        }
    }
}
