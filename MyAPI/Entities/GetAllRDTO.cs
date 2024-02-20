namespace MyAPI.Entities;

public record GetAllRDTO<T>(
  int totalOfRecords,
  IEnumerable<T> records
);
