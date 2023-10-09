-- Таблиця користувачів
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Email NVARCHAR(100) NOT NULL,
    Password NVARCHAR(200) NOT NULL,
    Name NVARCHAR(50) NOT NULL,
    Surname NVARCHAR(50) NOT NULL,
    BirthDate DATE,
    Currency NVARCHAR(3) NOT NULL,
    AverageIncome DECIMAL(10, 2)
);

-- Таблиця категорій
CREATE TABLE Categories (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER,
    Title NVARCHAR(50) NOT NULL,
    IsGeneral BIT NOT NULL, 
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- Таблиця записів про доходи та витрати
CREATE TABLE Transactions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    CategoryId UNIQUEIDENTIFIER,
    Amount DECIMAL(10, 2) NOT NULL,
    Date DATE,
    Title NVARCHAR(50) NOT NULL,
    Details NVARCHAR(255),
    Planned BIT NOT NULL,

    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
);

-- Таблиця записів про планування
CREATE TABLE Planning (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CategoryId UNIQUEIDENTIFIER NOT NULL,
    UserId UNIQUEIDENTIFIER NOT NULL,
    PercentageAmount DECIMAL(5, 2) NOT NULL,

    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
);
