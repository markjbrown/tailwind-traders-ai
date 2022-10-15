package Tailwind.Traders.Stock.Api.repositories;

import java.util.UUID;
import java.util.concurrent.ExecutionException;

import org.springframework.stereotype.Service;

import com.google.api.core.ApiFuture;
import com.google.cloud.firestore.DocumentReference;
import com.google.cloud.firestore.DocumentSnapshot;
import com.google.cloud.firestore.Firestore;
import com.google.cloud.firestore.WriteResult;
import com.google.firebase.cloud.FirestoreClient;

import Tailwind.Traders.Stock.Api.constant.CommonConstant;
import Tailwind.Traders.Stock.Api.models.StockItem;

@Service("GCP")
public class GCPStockItemRepository implements StockItemRepository {

	@Override
	public StockItem findByProductId(String id) {
		Firestore dbFirestore = FirestoreClient.getFirestore();
		DocumentReference documentReference = dbFirestore.collection(CommonConstant.COLLECTION_ID).document(id);
		ApiFuture<DocumentSnapshot> future = documentReference.get();
		DocumentSnapshot document;
		StockItem customer = null;
		try {
			document = future.get();
			if (document.exists()) {
				customer = document.toObject(StockItem.class);
			}
		} catch (InterruptedException | ExecutionException e) {
			e.printStackTrace();
		}
		return customer;
	}

	@Override
	public boolean update(StockItem stock) {
		Firestore dbFirestore = FirestoreClient.getFirestore();
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
		// TODO Auto-generated method stub
		return 0;
	}

	@Override
	public boolean save(StockItem stockItem) {
		Firestore dbFirestore = FirestoreClient.getFirestore();
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