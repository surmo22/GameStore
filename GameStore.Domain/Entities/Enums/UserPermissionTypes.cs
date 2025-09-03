namespace GameStore.Domain.Entities.Enums;

public enum UserPermissionTypes
{
    // User Management
    ManageUsers = 1,
    ManageRoles = 2,
    
    // Game Management
    ViewGames = 3,
    ManageEntities = 4,
    ViewDeletedGames = 5,
    EditDeletedGames = 6,
    
    // Order Management
    ViewOrders = 7,
    EditOrders = 8,
    ViewOrderHistory = 9,
    ChangeOrderStatus = 10,
    
    // Comment Management
    AddComments = 11,
    ManageComments = 12,
    ManageDeletedGameComments = 13,
    BanUsers = 14,
    
    // Notification
    ReceiveNotificationOnOrderStatusChange = 15,
}