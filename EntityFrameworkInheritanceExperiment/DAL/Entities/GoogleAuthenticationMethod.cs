﻿namespace EntityFrameworkInheritanceExperiment.DAL.Entities
{
    public class GoogleAuthenticationMethod : AuthenticationMethod
    {
        public string GoogleUserId { get; set; }
        public string Email { get; set; }
    }
}