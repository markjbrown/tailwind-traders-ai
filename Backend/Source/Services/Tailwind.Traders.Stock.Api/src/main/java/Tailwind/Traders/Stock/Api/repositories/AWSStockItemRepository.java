package Tailwind.Traders.Stock.Api.repositories;

import java.util.List;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;

import com.amazonaws.services.dynamodbv2.datamodeling.DynamoDBMapper;
import com.amazonaws.services.dynamodbv2.datamodeling.DynamoDBSaveExpression;
import com.amazonaws.services.dynamodbv2.datamodeling.DynamoDBScanExpression;
import com.amazonaws.services.dynamodbv2.model.AttributeValue;
import com.amazonaws.services.dynamodbv2.model.ExpectedAttributeValue;

import Tailwind.Traders.Stock.Api.config.DynamoDBConfiguration;
import Tailwind.Traders.Stock.Api.models.StockItem;

@Service("AWS")
public class AWSStockItemRepository implements StockItemRepository {

	private DynamoDBMapper dynamoDBMapper;

	public AWSStockItemRepository(@Value("${dynamic.db}") String dynaimc, DynamoDBConfiguration dynamoDBConfiguration) {
		if (dynaimc.equals("AWS")) {
			this.dynamoDBMapper = dynamoDBConfiguration.buildAmazonDynamoDB();
		}
	}

	@Override
	public StockItem findByProductId(Integer id) {
		List<StockItem> stocks = dynamoDBMapper.scan(StockItem.class, new DynamoDBScanExpression());
		return stocks.stream().filter(f->f.getProductId().equals(id)).findFirst().orElse(null);
	}

	@Override
	public boolean update(StockItem stock) {
		dynamoDBMapper.save(stock, new DynamoDBSaveExpression().withExpectedEntry("id",
				new ExpectedAttributeValue(new AttributeValue().withS(stock.getId()))));
		return true;
	}

	@Override
	public Integer count() {
		List<StockItem> stocks = dynamoDBMapper.scan(StockItem.class, new DynamoDBScanExpression());
		return stocks.size();
	}

	@Override
	public boolean save(StockItem stockItem) {
		dynamoDBMapper.save(stockItem);
		return true;
	}

}