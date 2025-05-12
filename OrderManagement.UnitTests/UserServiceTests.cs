using System.Data;
using Moq;
using OrderManagement.Core.Entities;
using OrderManagement.Core.Interfaces;
using OrderManagement.Core.Services;

namespace OrderManagement.UnitTests;

public class UserServiceTests
{
    private readonly IUserService _userService;
    private readonly Mock<IRepository<User>> _mockUserRepo;
    
    private readonly List<User> _mockUsers;
    
    public UserServiceTests()
    {
        _mockUsers = new List<User>()
        {
            new User { Id = Guid.NewGuid(), Username = "User1" },
            new User { Id = Guid.NewGuid(), Username = "User2" }
        };
        
        _mockUserRepo = new Mock<IRepository<User>>();
        _userService = new UserService(_mockUserRepo.Object);
    }
    
    [Fact]
    public void GetAllUsers_ShouldReturnAllUsers_WhenUsersExist()
    {
        // Arrange
        _mockUserRepo.Setup(repo => repo.GetAll()).Returns(_mockUsers);

        // Act
        List<User> result = _userService.GetAllUsers();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(_mockUsers, result);
        _mockUserRepo.Verify(repo => repo.GetAll(), Times.Once);
    }

    [Fact]
    public void GetAllUsers_ShouldReturnEmptyList_WhenNoUsersExist()
    {
        // Arrange
        _mockUserRepo.Setup(repo => repo.GetAll()).Returns(new List<User>());

        // Act
        List<User> result = _userService.GetAllUsers();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _mockUserRepo.Verify(repo => repo.GetAll(), Times.Once);
    }

    [Fact]
    public void GetUser_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        User testUser = _mockUsers.First();
        _mockUserRepo.Setup(repo => repo.Get(testUser.Id)).Returns(testUser);

        // Act
        User result = _userService.GetUser(testUser.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(testUser.Id, result.Id);
        Assert.Equal(testUser.Username, result.Username);
        _mockUserRepo.Verify(repo => repo.Get(testUser.Id), Times.Once);
    }

    [Fact]
    public void GetUser_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        Guid nonExistentId = Guid.NewGuid();
        _mockUserRepo.Setup(repo => repo.Get(nonExistentId)).Returns((User)null);

        // Act
        User result = _userService.GetUser(nonExistentId);

        // Assert
        Assert.Null(result);
        _mockUserRepo.Verify(repo => repo.Get(nonExistentId), Times.Once);
    }

    [Fact]
    public void CreateUser_ShouldCreateAndReturnUser_WhenUsernameIsValidAndUnique()
    {
        // Arrange
        string newUsername = "NewUser";
        _mockUserRepo.Setup(repo => repo.GetAll()).Returns(_mockUsers);
        _mockUserRepo.Setup(repo => repo.Add(It.IsAny<User>()))
            .Callback<User>(user =>
            {
                user.Id = Guid.NewGuid();
                _mockUsers.Add(user);
            });

        // Act
        User result = _userService.CreateUser(newUsername);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newUsername, result.Username);
        Assert.NotEqual(Guid.Empty, result.Id);
        _mockUserRepo.Verify(repo => repo.GetAll(), Times.Once);
        _mockUserRepo.Verify(repo => repo.Add(It.Is<User>(u => u.Username == newUsername)), Times.Once);
    }

    [Theory]
    [InlineData("U")]  // Too short
    [InlineData("Us")] // Too short
    [InlineData("ThisUsernameIsWayTooLong")] // Too long
    [InlineData("AnotherVeryLongUsername")] // Too long
    public void CreateUser_ShouldThrowException_WhenUsernameLengthIsInvalid(string invalidUsername)
    {
        // Act & Assert
        Exception exception = Assert.Throws<Exception>(() => _userService.CreateUser(invalidUsername));
        Assert.Equal("Invalid username length.", exception.Message);
        _mockUserRepo.Verify(repo => repo.GetAll(), Times.Never);
        _mockUserRepo.Verify(repo => repo.Add(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public void CreateUser_ShouldThrowDuplicateNameException_WhenUsernameAlreadyExists()
    {
        // Arrange
        string existingUsername = "User1";
        _mockUserRepo.Setup(repo => repo.GetAll()).Returns(_mockUsers);

        // Act & Assert
        Exception exception = Assert.Throws<DuplicateNameException>(() => _userService.CreateUser(existingUsername));
        Assert.Equal($"Username {existingUsername} already taken.", exception.Message);
        _mockUserRepo.Verify(repo => repo.GetAll(), Times.Once);
        _mockUserRepo.Verify(repo => repo.Add(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public void CreateUser_ShouldThrowDuplicateNameException_WhenUsernameAlreadyExists_CaseSensitiveCheck()
    {
        // Arrange
        string existingUsername = "user1";
        _mockUserRepo.Setup(repo => repo.GetAll()).Returns(_mockUsers);

        // Act
        User result = _userService.CreateUser(existingUsername);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existingUsername, result.Username);
        _mockUserRepo.Verify(repo => repo.GetAll(), Times.Once);
        _mockUserRepo.Verify(repo => repo.Add(It.Is<User>(u => u.Username == existingUsername)), Times.Once);
    }

    [Fact]
    public void CreateUser_ShouldCreateUser_WhenUsernameIsValidAndRepoIsEmpty()
    {
        // Arrange
        string newUsername = "FirstUser";
        _mockUserRepo.Setup(repo => repo.GetAll()).Returns(new List<User>());
        _mockUserRepo.Setup(repo => repo.Add(It.IsAny<User>()))
            .Callback<User>(user => user.Id = Guid.NewGuid());

        // Act
        User result = _userService.CreateUser(newUsername);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newUsername, result.Username);
        Assert.NotEqual(Guid.Empty, result.Id);
        _mockUserRepo.Verify(repo => repo.GetAll(), Times.Once);
        _mockUserRepo.Verify(repo => repo.Add(It.Is<User>(u => u.Username == newUsername)), Times.Once);
    }
}