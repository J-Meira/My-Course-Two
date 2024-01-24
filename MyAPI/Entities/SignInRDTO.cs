namespace MyAPI.Entities;

public record SignInRDTO(
  string token,
  DateTime expireIn,
  UserSignIn user
);