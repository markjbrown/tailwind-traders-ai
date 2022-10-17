package Tailwind.Traders.Stock.Api.controller;

import java.io.IOException;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;
import org.springframework.beans.factory.BeanFactory;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;

import Tailwind.Traders.Stock.Api.StockProduct;
import Tailwind.Traders.Stock.Api.models.StockItem;
import Tailwind.Traders.Stock.Api.repositories.StockItemRepository;

@RestController
public class StockController {

	private StockItemRepository stockItemRepository;

	public StockController(BeanFactory beanFactory, @Value("${dynamic.db}") String dynaimc) {
		if (dynaimc.equals("AWS"))
			this.stockItemRepository = beanFactory.getBean("AWS", StockItemRepository.class);
		else if (dynaimc.equals("GCP")) {
			this.stockItemRepository = beanFactory.getBean("GCP", StockItemRepository.class);
		} else if (dynaimc.equals("AZURE")) {
			this.stockItemRepository = beanFactory.getBean("AZURE", StockItemRepository.class);
		} else {
			System.out.println("No Database Selected");
			System.exit(1);
		}
	}

	private final Logger log = LogManager.getLogger(StockController.class);

	@GetMapping(value = "/v1/stock/{id}")
	public ResponseEntity<StockProduct> StockProduct(@PathVariable(value = "id", required = true) Integer id)
			throws IOException, Exception {
		StockItem stock = stockItemRepository.findByProductId(id);

		if (stock == null) {
			log.debug("Not found stock for product " + id);
			return new ResponseEntity<StockProduct>(HttpStatus.NOT_FOUND);
		}

		StockProduct response = new StockProduct();
		response.setId(stock.getId());
		response.setProductId(stock.getProductId());
		response.setProductStock(stock.getStockCount());
		return new ResponseEntity<StockProduct>(response, HttpStatus.OK);
	}

	@PostMapping("/v1/consumptions/stock/{id}")
	public ResponseEntity<StockItem> decreaseStock(@PathVariable Integer id) {
		StockItem stock = stockItemRepository.findByProductId(id);
		if (stock == null) {
			log.debug("Not found stock for product " + id);
			return new ResponseEntity<StockItem>(HttpStatus.NOT_FOUND);
		}
		int currentStock = stock.getStockCount();
		if (currentStock > 0) {
			stock.setStockCount(currentStock - 1);
			stockItemRepository.update(stock);
			return new ResponseEntity<StockItem>(stock, HttpStatus.OK);
		}
		return new ResponseEntity<StockItem>(HttpStatus.BAD_REQUEST);

	}

	@PostMapping("/v1/consumptions/stock")
	public ResponseEntity<StockItem> add(@RequestBody StockItem stockItem) {
		stockItemRepository.save(stockItem);
		return new ResponseEntity<StockItem>(stockItem, HttpStatus.OK);
	}

}
