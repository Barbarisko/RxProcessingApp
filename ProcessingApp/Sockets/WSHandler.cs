using System;
using System.Reactive.Linq;
using Microsoft.AspNetCore.Mvc;
using ProcessingApp.Price_Service_Idl.Src.Service;
using ProcessingApp.Trade_Service_Idl.Src.Service;

namespace ProcessingApp.Sockets
{
    public class WsHandler
    {
        private readonly IPriceService _priceService;
        private readonly ITradeService _tradeService;

        public WsHandler(
            IPriceService priceService,
            ITradeService tradeService)
        {
            _priceService = priceService;
            _tradeService = tradeService;
        }

        [HttpGet]
        public IObservable<dynamic> Handle(IObservable<string> inbound)
        {
            return inbound.Let(HandleRequestedAveragePriceIntervalValue)
                .Let(_priceService.PricesStream)
                .Merge<dynamic>(_tradeService.TradesStream());
        }

        private static IObservable<long> HandleRequestedAveragePriceIntervalValue(
            IObservable<string> requestedInterval)
        {
            var checkedInterval = requestedInterval
                .Where(i => long.TryParse(i, out var o) && o > 0 && o <= 60)
                .Select(long.Parse);

            return checkedInterval;
        }
    }
}