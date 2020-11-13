# PriceTracker Telegram Bot
### About
Telegram бот на C#, ASP.NET Core 3.1, с использованием клиента для *Telegram Bot API* [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot). Хостится на сервере Ubuntu 20.04 + Nginx. Получает *update*'ы через *Webhook*.

*PriceTrackerBot* умеет добавлять товары по ссылке в корзину, присылать уведомления об изменении их цены, показывать содержимое корзины и удалять товары.

В данный момент есть возможность добавлять товары из *PULL&BEAR* и *Bershka*.

*Name*: *PriceTrackerBot*

*Username*: *@sales_tracker_bot*

* * *
### Setup
+ Мой [гайд](https://salmonsenya.github.io/DiscourteousBotWebhook/) о том, как написать Telegram бот на ASP.Net Core и раскатить его на Linux сервере
+ Хорошая [статья](https://www.digitalocean.com/community/tutorials/how-to-deploy-an-asp-net-core-application-with-mysql-server-using-nginx-on-ubuntu-18-04) по деплою ASP.NET Core приложения с MySQL с использованием Nginx и Ubuntu.
+ [Руководство](https://docs.microsoft.com/ru-ru/aspnet/core/data/ef-mvc/migrations?view=aspnetcore-3.1) по использованию ASP.NET MVC с EF Core
+ Объемный [гайд](https://core.telegram.org/bots/webhooks#testing-your-bot-with-updates) по настройке *webhook*'а: чеклист и примеры *json*-объектов *update*'ов.
