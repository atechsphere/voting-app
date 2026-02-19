-- Initialize voting database
CREATE DATABASE IF NOT EXISTS votingdb;
USE votingdb;

-- Grant privileges to voting user
GRANT ALL PRIVILEGES ON votingdb.* TO 'votinguser'@'%';
FLUSH PRIVILEGES;

-- The tables will be created by Entity Framework migrations
SELECT 'Database initialized successfully' AS Status;
