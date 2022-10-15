package Tailwind.Traders.Stock.Api.repositories;

import Tailwind.Traders.Stock.Api.models.StockItem;

public interface StockItemRepository {
	
	boolean update(StockItem stock);

	Integer count();

	boolean save(StockItem stockItem);

	StockItem findByProductId(String id);
}
