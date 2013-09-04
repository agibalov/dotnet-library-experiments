﻿using System;
using System.Collections.Generic;
using System.Linq;
using EntityFrameworkInheritanceExperiment.DAL;
using EntityFrameworkInheritanceExperiment.DAL.Entities;
using EntityFrameworkInheritanceExperiment.DTO;

namespace EntityFrameworkInheritanceExperiment.Service.Domain
{
    public class DDDUser
    {
        private readonly UserContext _context;
        private readonly User _user;

        public DDDUser(UserContext context, User user)
        {
            _context = context;
            _user = user;
        }

        public int UserId
        {
            get { return _user.UserId; }
        }

        public bool UserHasPasswordSet()
        {
            return _context.AuthenticationMethods
                .OfType<PasswordAuthenticationMethod>()
                .Any(am => am.UserId == _user.UserId);
        }

        public IList<PasswordAuthenticationMethod> UserGetPasswordAuthenticationMethods()
        {
            return _context.AuthenticationMethods
                .OfType<PasswordAuthenticationMethod>()
                .Where(am => am.UserId == _user.UserId)
                .ToList();
        }

        public void UserAddEmailAddress(string email)
        {
            var emailAddress = new EmailAddress
            {
                Email = email,
                User = _user
            };
            _context.EmailAddresses.Add(emailAddress);
        }

        public void UserAddPasswordAuthenticationMethod(string password)
        {
            var passwordAuthMethod = new PasswordAuthenticationMethod
            {
                Password = password,
                User = _user
            };
            _context.AuthenticationMethods.Add(passwordAuthMethod);
        }

        public void UserAddGoogleAuthenticationMethod(string googleUserId)
        {
            var googleAuthMethod = new GoogleAuthenticationMethod
            {
                GoogleUserId = googleUserId,
                User = _user
            };
            _context.AuthenticationMethods.Add(googleAuthMethod);
        }

        public void UserAddFacebookAuthenticationMethod(string facebookUserId)
        {
            var facebookAuthMethod = new FacebookAuthenticationMethod
            {
                FacebookUserId = facebookUserId,
                User = _user
            };
            _context.AuthenticationMethods.Add(facebookAuthMethod);
        }

        public void UserAddTwitterAuthenticationMethod(string twitterUserId, string twitterDisplayName)
        {
            var twitterAuthMethod = new TwitterAuthenticationMethod
            {
                TwitterUserId = twitterUserId,
                TwitterDisplayName = twitterDisplayName,
                User = _user
            };
            _context.AuthenticationMethods.Add(twitterAuthMethod);
        }

        #region Entity -> DTO mapping should not be here
        public UserDTO AsUserDTO()
        {
            return new UserDTO
            {
                UserId = _user.UserId,
                AuthenticationMethods = _user.AuthenticationMethods
                    .Select(MapAuthenticationMethodToAuthenticationMethodDTO)
                    .ToList(),
                EmailAddresses = _user.EmailAddresses
                    .Select(emailAddress => new EmailAddressDTO
                    {
                        EmailAddressId = emailAddress.EmailAddressId,
                        Email = emailAddress.Email
                    }).ToList()
            };
        }

        private static AuthenticationMethodDTO MapAuthenticationMethodToAuthenticationMethodDTO(AuthenticationMethod authenticationMethod)
        {
            if (authenticationMethod is PasswordAuthenticationMethod)
            {
                return MapEmailPasswordAuthenticationMethodToEmailPasswordAuthenticationMethodDTO((PasswordAuthenticationMethod)authenticationMethod);
            }

            if (authenticationMethod is GoogleAuthenticationMethod)
            {
                return MapGoogleAuthenticationMethodToGoogleAuthenticationMethodDTO((GoogleAuthenticationMethod)authenticationMethod);
            }

            if (authenticationMethod is FacebookAuthenticationMethod)
            {
                return MapFacebookAuthenticationMethodToFacebookAuthenticationMethodDTO((FacebookAuthenticationMethod)authenticationMethod);
            }

            if (authenticationMethod is TwitterAuthenticationMethod)
            {
                return MapTwitterAuthenticationMethodToTwitterAuthenticationMethodDTO((TwitterAuthenticationMethod)authenticationMethod);
            }

            throw new NotImplementedException();
        }

        private static PasswordAuthenticationMethodDTO
            MapEmailPasswordAuthenticationMethodToEmailPasswordAuthenticationMethodDTO(
            PasswordAuthenticationMethod authenticationMethod)
        {
            return new PasswordAuthenticationMethodDTO
            {
                AuthenticationMethodId = authenticationMethod.AuthenticationMethodId,
                Password = authenticationMethod.Password
            };
        }

        private static GoogleAuthenticationMethodDTO MapGoogleAuthenticationMethodToGoogleAuthenticationMethodDTO(GoogleAuthenticationMethod authenticationMethod)
        {
            return new GoogleAuthenticationMethodDTO
            {
                AuthenticationMethodId = authenticationMethod.AuthenticationMethodId,
                GoogleUserId = authenticationMethod.GoogleUserId,
            };
        }

        private static FacebookAuthenticationMethodDTO MapFacebookAuthenticationMethodToFacebookAuthenticationMethodDTO(FacebookAuthenticationMethod authenticationMethod)
        {
            return new FacebookAuthenticationMethodDTO
            {
                AuthenticationMethodId = authenticationMethod.AuthenticationMethodId,
                FacebookUserId = authenticationMethod.FacebookUserId,
            };
        }

        private static TwitterAuthenticationMethodDTO MapTwitterAuthenticationMethodToTwitterAuthenticationMethodDTO(
            TwitterAuthenticationMethod authenticationMethod)
        {
            return new TwitterAuthenticationMethodDTO
            {
                AuthenticationMethodId = authenticationMethod.AuthenticationMethodId,
                TwitterUserId = authenticationMethod.TwitterUserId,
            };
        }
        #endregion
    }
}