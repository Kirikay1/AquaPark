CREATE DATABASE AquaPark;
GO

USE AquaPark;
GO

CREATE TABLE Roles (
    role_id INT IDENTITY(1,1) PRIMARY KEY,
    role_name NVARCHAR(50) NOT NULL UNIQUE
);
GO

CREATE TABLE Users (
    user_id INT IDENTITY(1,1) PRIMARY KEY,
    login NVARCHAR(100) NOT NULL UNIQUE,
    password NVARCHAR(100) NOT NULL,
    full_name NVARCHAR(150) NOT NULL,
    email NVARCHAR(100),
    phone NVARCHAR(20),
    role_id INT NOT NULL,
    is_active BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (role_id) REFERENCES Roles(role_id)
);
GO

CREATE TABLE Employees (
    employee_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL UNIQUE,
    position NVARCHAR(100) NOT NULL,
    hire_date DATE NOT NULL,
    salary DECIMAL(10,2),
    FOREIGN KEY (user_id) REFERENCES Users(user_id),
    CHECK (salary >= 0)
);
GO

CREATE TABLE Clients (
    client_id INT IDENTITY(1,1) PRIMARY KEY,
    full_name NVARCHAR(150) NOT NULL,
    birth_date DATE,
    phone NVARCHAR(20),
    email NVARCHAR(100)
);
GO

CREATE TABLE Zones (
    zone_id INT IDENTITY(1,1) PRIMARY KEY,
    zone_name NVARCHAR(100) NOT NULL UNIQUE,
    description NVARCHAR(250)
);
GO

CREATE TABLE Attractions (
    attraction_id INT IDENTITY(1,1) PRIMARY KEY,
    attraction_name NVARCHAR(150) NOT NULL,
    zone_id INT NOT NULL,
    description NVARCHAR(250),
    age_limit INT NOT NULL DEFAULT 0,
    height_limit INT,
    is_active BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (zone_id) REFERENCES Zones(zone_id),
    CHECK (age_limit >= 0),
    CHECK (height_limit IS NULL OR height_limit >= 0)
);
GO

CREATE TABLE TicketTypes (
    ticket_type_id INT IDENTITY(1,1) PRIMARY KEY,
    ticket_name NVARCHAR(100) NOT NULL UNIQUE,
    description NVARCHAR(250),
    price DECIMAL(10,2) NOT NULL,
    duration_hours INT NOT NULL,
    CHECK (price > 0),
    CHECK (duration_hours > 0)
);
GO

CREATE TABLE Tickets (
    ticket_id INT IDENTITY(1,1) PRIMARY KEY,
    ticket_type_id INT NOT NULL,
    client_id INT,
    purchase_date DATETIME NOT NULL DEFAULT GETDATE(),
    visit_date DATE NOT NULL,
    status NVARCHAR(50) NOT NULL DEFAULT N'Активен',
    FOREIGN KEY (ticket_type_id) REFERENCES TicketTypes(ticket_type_id),
    FOREIGN KEY (client_id) REFERENCES Clients(client_id)
);
GO

CREATE TABLE Sales (
    sale_id INT IDENTITY(1,1) PRIMARY KEY,
    ticket_id INT NOT NULL,
    employee_id INT NOT NULL,
    sale_date DATETIME NOT NULL DEFAULT GETDATE(),
    total_amount DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (ticket_id) REFERENCES Tickets(ticket_id),
    FOREIGN KEY (employee_id) REFERENCES Employees(employee_id),
    CHECK (total_amount > 0)
);
GO

CREATE TABLE Payments (
    payment_id INT IDENTITY(1,1) PRIMARY KEY,
    sale_id INT NOT NULL,
    payment_date DATETIME NOT NULL DEFAULT GETDATE(),
    amount DECIMAL(10,2) NOT NULL,
    payment_method NVARCHAR(50) NOT NULL,
    payment_status NVARCHAR(50) NOT NULL DEFAULT N'Оплачено',
    FOREIGN KEY (sale_id) REFERENCES Sales(sale_id),
    CHECK (amount > 0)
);
GO

CREATE TABLE Visits (
    visit_id INT IDENTITY(1,1) PRIMARY KEY,
    ticket_id INT NOT NULL,
    entry_time DATETIME NOT NULL DEFAULT GETDATE(),
    exit_time DATETIME,
    FOREIGN KEY (ticket_id) REFERENCES Tickets(ticket_id),
    CHECK (exit_time IS NULL OR exit_time >= entry_time)
);
GO

CREATE TABLE AttractionSchedule (
    schedule_id INT IDENTITY(1,1) PRIMARY KEY,
    attraction_id INT NOT NULL,
    work_date DATE NOT NULL,
    start_time TIME NOT NULL,
    end_time TIME NOT NULL,
    status NVARCHAR(50) NOT NULL DEFAULT N'Работает',
    FOREIGN KEY (attraction_id) REFERENCES Attractions(attraction_id),
    CHECK (end_time > start_time)
);
GO