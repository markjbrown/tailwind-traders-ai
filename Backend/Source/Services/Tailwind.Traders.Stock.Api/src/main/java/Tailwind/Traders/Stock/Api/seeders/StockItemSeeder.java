package Tailwind.Traders.Stock.Api.seeders;

import java.io.BufferedReader;
import java.io.IOException;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.List;

import javax.annotation.PostConstruct;

import org.springframework.beans.factory.BeanFactory;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Component;

import com.opencsv.bean.CsvToBeanBuilder;

import Tailwind.Traders.Stock.Api.StockProduct;
import Tailwind.Traders.Stock.Api.models.StockItem;
import Tailwind.Traders.Stock.Api.repositories.StockItemRepository;

@Component
public class StockItemSeeder {

	private StockItemRepository stockItemRepository;

	public StockItemSeeder(BeanFactory beanFactory, @Value("${dynamic.db}") String dynamic) {
		if (dynamic.equals("AWS"))
			this.stockItemRepository = beanFactory.getBean("AWS", StockItemRepository.class);
		else if (dynamic.equals("GCP")) {
			this.stockItemRepository = beanFactory.getBean("GCP", StockItemRepository.class);
		} else if (dynamic.equals("AZURE")) {
			this.stockItemRepository = beanFactory.getBean("AZURE", StockItemRepository.class);
		} else {
			System.out.println("No Database Selected");
			System.exit(1);
		}
	}
	
	@PostConstruct
	public void initialize() {
		seed();
	}

	public void seed() {
		boolean alreadySeeded = stockItemRepository.count() > 0;
		if (alreadySeeded) {
			return;
		}
		BufferedReader reader;
		List<StockProduct> allStock = null;
		try {
			reader = Files.newBufferedReader(Paths.get("setup/StockProduct.csv"), StandardCharsets.UTF_8);
			allStock = new CsvToBeanBuilder<StockProduct>(reader).withType(StockProduct.class).build().parse();
		} catch (IOException e) {
			e.printStackTrace();
		}
		List<Integer> setted = new ArrayList<Integer>();

		for (StockProduct stock : allStock) {
			StockItem item = new StockItem();
			item.setProductId(stock.getProductId());
			item.setStockCount(stock.getProductStock());
			setted.add(stock.getProductId());
			stockItemRepository.save(item);
		}

		// For all other products up to MAX_PRODUCT_ID set a 100 stock units

		String mpid = System.getenv("MAX_PRODUCT_ID");
		int defaultStock = 60;
		int maxpid = 0;
		try {
			maxpid = Integer.parseInt(mpid);
		} catch (NumberFormatException ex) {
			maxpid = 250;
		}

		for (int idx = 1; idx <= maxpid; idx++) {
			if (!setted.contains(idx)) {
				StockItem item = new StockItem();
				item.setProductId(idx);
				item.setStockCount(defaultStock);
				stockItemRepository.save(item);
			}
		}
	}
}
