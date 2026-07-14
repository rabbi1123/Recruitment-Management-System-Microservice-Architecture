namespace Job.Infrastructure.Sql
{
    public static class GenericInterfaceSql
    {
        public const string GenericInterfaceMaster_GetByMenuId = @"
            SELECT MenuId, InterfaceName, DisplayName, UseBaseUrl, GetEndpointUrl, ParameterColumns, MasterRowNum, MasterColNum, AllowInsert, AllowUpdate, AllowDelete, HasGrid, InsertEndpointUrl, UpdateEndpointUrl, DeleteEndpointUrl
            FROM     GenericInterfaceMaster
            WHERE  (MenuId = @MenuId)";

        public const string GenericInterfaceDetail_GetByMenuId = @"
            SELECT DetailId, MenuId, ParentId, ColumnName, Label, EntryType, Scale, Sys, Required, Visible, Enable, Seq, Tooltip, CellWidth, HasFinder, UseBaseUrl, FinderEndpointUrl, FinderHeaderColumns, FinderDisplayColumns, 
                              FinderValueColumn, ComboDisplayColumn, FinderDisplayOthersColumn, HasSelectionChangeMethod, ComboSelectionChangeEndpointUrl, HasDefault, DefaultValue, ParameterValue
            FROM     GenericInterfaceDetail
            WHERE  (MenuId = @MenuId)";

        public const string GenericInterfaceDetailGrid_GetByMenuId = @"
            SELECT DetailGridId, MenuId, ColumnsName, ColumnsHeader, ColumnsWidth, ColumnsAlign, ColumnsType, ColumnsSorting, ColumnsHidden, PrimaryKeyColumn, ParentColumn, SessionName, RemoveSession, UseBaseUrl, 
                              GetEndpointUrl, InsertEndpointUrl, UpdateEndpointUrl, DeleteEndpointUrl, HasSearch
            FROM     GenericInterfaceDetailGrid
            WHERE  (MenuId = @MenuId)";

        public const string GenericInterfaceDetailGridColumn_GetByMenuId = @"
            SELECT DetailGridColumnId, DetailGridId, MenuId, ColumnName, HasFinder, UseBaseUrl, FinderEndpointUrl, FinderHeaderColumns, FinderDisplayColumns, FinderValueColumn, ComboDisplayColumn, FinderDisplayOthersColumn, 
                              HasSelectionChangeMethod, ComboSelectionChangeEndpointUrl, HasDefault, DefaultValue, ParameterValue, ParentColumnInMaster, ParentColumnName
            FROM     GenericInterfaceDetailGridColumn
            WHERE  (MenuId = @MenuId)";
    }
}
