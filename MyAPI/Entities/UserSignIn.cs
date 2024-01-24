namespace MyAPI.Entities;

public record UserSignIn(
  string userId,
  string userName,
  string? employeeRegistration
);
