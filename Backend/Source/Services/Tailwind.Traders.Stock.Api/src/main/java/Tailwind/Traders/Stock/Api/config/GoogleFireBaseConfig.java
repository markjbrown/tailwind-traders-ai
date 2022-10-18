package Tailwind.Traders.Stock.Api.config;

import java.io.InputStream;

import javax.annotation.PostConstruct;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Configuration;

import com.google.auth.oauth2.GoogleCredentials;
import com.google.firebase.FirebaseApp;
import com.google.firebase.FirebaseOptions;
import org.json.JSONObject;


@Configuration
public class GoogleFireBaseConfig {

	@Value("${dynamic.db}")
	private String dynaimc;

	@PostConstruct
	public void initialize() {
		if (dynaimc.equals("GCP")) {
			try {
				JSONObject cpResource=new JSONObject();
				cpResource.put("type", @Value("${gcp.type}")); 
				
				
				InputStream cpResource = this.getClass().getClassLoader().getResourceAsStream("serviceKey.json");
				FirebaseOptions options = FirebaseOptions.builder()
						.setCredentials(GoogleCredentials.fromStream(cpResource)).build();
				FirebaseApp.initializeApp(options);
			} catch (Exception e) {
				e.printStackTrace();
			}
		}

	}
	
	

}
