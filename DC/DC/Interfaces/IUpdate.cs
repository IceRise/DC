using System.Data;
using System.Windows.Controls;

namespace DC
{
    interface IUpdate
    {
         void Update(BaseDataTable sourceTable, ItemCollection connectionArray);
         string BuildUpdateCommand(DataRow dr);
         string BuildInsertCommand(DataRow dr);
    }
}
