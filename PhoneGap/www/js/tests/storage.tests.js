Tests.prototype.StorageTests = function() 
{
  module("HTML 5 Storage");
  test("should exist", function() {
    expect(1);
    ok(typeof(window.openDatabase) == "function", "Database is defined");
  });
  test("Should open a database", function() {
    var db = openDatabase("Database", "1.0", "HTML5 Database API example", 200000);
    ok(db != null, "Database should be opened");
  });
}
