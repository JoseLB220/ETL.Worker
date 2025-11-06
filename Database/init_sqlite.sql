CREATE TABLE IF NOT EXISTS staging (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Source TEXT,
    UserName TEXT,
    Rating INTEGER,
    Comment TEXT,
    CreatedAt TEXT,
    InsertedAt TEXT DEFAULT (datetime('now'))
);
