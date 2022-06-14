using DbConnect;
using DbConnect.Items;

Console.WriteLine(Styles.Update(7,"Test4"));

DbConnection.Stop();
while(DbConnection.IsConnected)
{
    DbConnection.Stop();
}