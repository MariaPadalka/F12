-- Таблиця користувачів
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Email NVARCHAR(100) NOT NULL,
    Password NVARCHAR(200) NOT NULL,
    Name NVARCHAR(50) NOT NULL,
    Surname NVARCHAR(50) NOT NULL,
    BirthDate DATE,
    Currency NVARCHAR(3) NOT NULL
);

-- Таблиця категорій
CREATE TABLE Categories (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER,
    Title NVARCHAR(50) NOT NULL,
    IsGeneral BIT NOT NULL, 
    PercentageAmount DECIMAL(5, 2) NOT NULL,
    Type nvarchar(50) NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    CHECK (Type IN (N'Дохід', N'Витрати', N'Баланс')) 
);

-- Таблиця записів про доходи та витрати
CREATE TABLE Transactions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    From_category UNIQUEIDENTIFIER,
    To_category UNIQUEIDENTIFIER,
    Amount DECIMAL(10, 2) NOT NULL,
    Date DATE,
    Title NVARCHAR(50) NOT NULL,
    Details NVARCHAR(255),
    Planned BIT NOT NULL,

    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (From_category) REFERENCES Categories(Id),
    FOREIGN KEY (To_category) REFERENCES Categories(Id)
);