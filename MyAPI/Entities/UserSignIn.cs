using MyAPI.Controllers;

namespace MyAPI.Entities;

public record UserSignIn(
  string userId,
  string userName,
  UserType userType
);
