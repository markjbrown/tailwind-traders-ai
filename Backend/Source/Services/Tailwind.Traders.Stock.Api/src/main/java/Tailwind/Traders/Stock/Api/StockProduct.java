package Tailwind.Traders.Stock.Api;

import com.opencsv.bean.CsvBindByName;

public class StockProduct {
	
	private String id;
	
	@CsvBindByName
	private int productId;

	@CsvBindByName
	private int productStock;

	public StockProduct() {
	}

	public StockProduct(String id, int productId, int productStock) {
		this.id = id;
		this.productId = productId;
		this.productStock = productStock;
	}

	public String getId() {
		return id;
	}

	public void setId(String id) {
		this.id = id;
	}

	public int getProductId() {
		return productId;
	}

	public void setProductId(int productId) {
		this.productId = productId;
	}

	public int getProductStock() {
		return productStock;
	}

	public void setProductStock(int productStock) {
		this.productStock = productStock;
	}

}
