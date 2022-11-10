package Tailwind.Traders.Stock.Api.repositories;

import java.util.UUID;
import java.util.concurrent.ExecutionException;

import org.springframework.stereotype.Service;

import com.google.api.core.ApiFuture;
import com.google.cloud.firestore.DocumentSnapshot;
import com.google.cloud.firestore.Firestore;
import com.google.cloud.firestore.FirestoreOptions;
import com.google.cloud.firestore.Query;
import com.google.cloud.firestore.QuerySnapshot;
import com.google.cloud.firestore.WriteResult;

import Tailwind.Traders.Stock.Api.constant.CommonConstant;
import Tailwind.Traders.Stock.Api.models.StockItem;

@Service("GCP")
public class GCPStockItemRepository implements StockItemRepository {

	@Override
	public StockItem findByProductId(Integer productId) {
		Firestore dbFirestore = FirestoreOptions.getDefaultInstance().getService();
		Query documentReference = dbFirestore.collection(CommonConstant.COLLECTION_ID).whereEqualTo("productId", productId);
		ApiFuture<QuerySnapshot> future = documentReference.get();
		StockItem stock = null;
		try {
			QuerySnapshot queryShapShot = future.get();
			if (!queryShapShot.isEmpty() && !queryShapShot.getDocuments().isEmpty()) {
				DocumentSnapshot document = queryShapShot.getDocuments().get(0);
				if (document.exists()) {
					stock = document.toObject(StockItem.class);
				}
			}
		} catch (InterruptedException | ExecutionException e) {
			e.printStackTrace();
		}
		return stock;

	}

	@Override
	public boolean update(StockItem stock) {
		Firestore dbFirestore = FirestoreOptions.getDefaultInstance().getService();
		ApiFuture<WriteResult> collectionsApiFuture = dbFirestore.collection(CommonConstant.COLLECTION_ID)
				.document(stock.getId()).set(stock);
		try {
			collectionsApiFuture.get().getUpdateTime().toString();
		} catch (InterruptedException | ExecutionException e) {
			e.printStackTrace();
		}
		return collectionsApiFuture.isDone();
	}

	@Override
	public Integer count() {
		Firestore dbFirestore = FirestoreOptions.getDefaultInstance().getService();
		Query documentReference = dbFirestore.collection(CommonConstant.COLLECTION_ID).select("productId");
		ApiFuture<QuerySnapshot> future = documentReference.get();
		try {
			QuerySnapshot queryShapShot = future.get();
			if (!queryShapShot.isEmpty() && !queryShapShot.getDocuments().isEmpty()) {
				return queryShapShot.getDocuments().size();
			}
		} catch (InterruptedException | ExecutionException e) {
			e.printStackTrace();
		}
		return 0;
	}

	@Override
	public boolean save(StockItem stockItem) {
		Firestore dbFirestore = FirestoreOptions.getDefaultInstance().getService();
		stockItem.setId(UUID.randomUUID().toString());
		ApiFuture<WriteResult> collectionsApiFuture = dbFirestore.collection(CommonConstant.COLLECTION_ID)
				.document(stockItem.getId()).set(stockItem);
		try {
			collectionsApiFuture.get().getUpdateTime().toString();
		} catch (InterruptedException | ExecutionException e) {
			e.printStackTrace();
		}
		return collectionsApiFuture.isDone();
	}

}