using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using MyShop.Controllers;
using MyShop.DAL;
using MyShop.Models;
using MyShop.ViewModels;

namespace MyShop.Tests.Controllers;

public class ItemControllerTests
{
    [Fact]
    public async Task Table_WhenItemsExist_ReturnsViewResultWithItems()
    {
        // Arrange
        var itemList = new List<Item>
        {
            new Item
            {
                ItemId = 1,
                Name = "Fried Chicken Wing",
                Price = 20,
                Description = "Delicious spicy chicken wing",
                ImageUrl = "/images/chickenwing.jpg"
            },
            new Item
            {
                ItemId = 2,
                Name = "Brown Cheese",
                Price = 20,
                Description = "Typical Norwegian cheese",
                ImageUrl = "/images/brunost.jpg"
            }
        };

        var mockItemRepository = Substitute.For<IItemRepository>();
        mockItemRepository.GetAll().Returns(itemList);

        var mockLogger = Substitute.For<ILogger<ItemController>>();
        var itemController = new ItemController(mockItemRepository, mockLogger);

        // Act
        var result = await itemController.Table();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var itemsViewModel = Assert.IsAssignableFrom<ItemsViewModel>(viewResult.ViewData.Model);
        Assert.Equal(2, itemsViewModel.Items.Count());
        Assert.Equal(itemList, itemsViewModel.Items);
    }

    [Fact]
    public async Task Create_WhenCreationFailed_ReturnsViewResultWithItem()
    {
        // Arrange
        var testItem = new Item
        {
            ItemId = 1,
            Name = "Spicy Chicken Wing",
            Price = 20,
            Description = "Delicious spicy chicken wing",
            ImageUrl = "/images/chickenwing.jpg"
        };

        var mockItemRepository = Substitute.For<IItemRepository>();
        mockItemRepository.Create(testItem).Returns(false);

        var mockLogger = Substitute.For<ILogger<ItemController>>();
        var itemController = new ItemController(mockItemRepository, mockLogger);

        // Act
        var result = await itemController.Create(testItem);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var viewItem = Assert.IsAssignableFrom<Item>(viewResult.ViewData.Model);
        Assert.Equal(testItem, viewItem);
    }    
}