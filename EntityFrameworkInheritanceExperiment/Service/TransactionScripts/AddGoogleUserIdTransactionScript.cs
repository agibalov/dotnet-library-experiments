﻿using EntityFrameworkInheritanceExperiment.DAL;
using EntityFrameworkInheritanceExperiment.DTO;
using EntityFrameworkInheritanceExperiment.Service.Configuration;
using EntityFrameworkInheritanceExperiment.Service.Domain;
using EntityFrameworkInheritanceExperiment.Service.Exceptions;
using EntityFrameworkInheritanceExperiment.Service.Mappers;

namespace EntityFrameworkInheritanceExperiment.Service.TransactionScripts
{
    [TransactionScript]
    public class AddGoogleUserIdTransactionScript
    {
        private readonly UserToUserDTOMapper _userToUserDtoMapper;
        private readonly UserRepository _userRepository;
        private readonly UserService _userService;

        public AddGoogleUserIdTransactionScript(
            UserToUserDTOMapper userToUserDtoMapper, 
            UserRepository userRepository,
            UserService userService)
        {
            _userToUserDtoMapper = userToUserDtoMapper;
            _userRepository = userRepository;
            _userService = userService;
        }

        public UserDTO AddGoogleUserId(UserContext context, int userId, string googleUserId, string email)
        {
            var user = _userRepository.FindUserByIdOrThrow(context, userId);

            var somebodyWhoAlreadyHasThisGoogleUserId = _userRepository.FindUserByGoogleUserId(context, googleUserId);
            if (somebodyWhoAlreadyHasThisGoogleUserId != null)
            {
                if (somebodyWhoAlreadyHasThisGoogleUserId.UserId != user.UserId)
                {
                    throw new GoogleUserIdAlreadyUsedException();
                }
            }
            else
            {
                _userService.UserAddGoogleAuthenticationMethod(context, user, googleUserId);
            }

            var somebodyWhoAlreadyHasThisEmail = _userRepository.FindUserByEmail(context, email);
            if (somebodyWhoAlreadyHasThisEmail != null)
            {
                if (somebodyWhoAlreadyHasThisEmail.UserId != user.UserId)
                {
                    throw new EmailAlreadyUsedException();
                }
            }
            else
            {
                _userService.UserAddEmailAddress(context, user, email);
            }

            context.SaveChanges();

            return _userToUserDtoMapper.MapUserToUserDTO(user);
        }
    }
}