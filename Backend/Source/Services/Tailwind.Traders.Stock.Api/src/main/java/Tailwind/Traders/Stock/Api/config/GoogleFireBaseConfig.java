package Tailwind.Traders.Stock.Api.config;

import java.io.FileInputStream;

import javax.annotation.PostConstruct;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Configuration;

import com.google.auth.oauth2.GoogleCredentials;
import com.google.firebase.FirebaseApp;
import com.google.firebase.FirebaseOptions;

@Configuration
public class GoogleFireBaseConfig {

	@Value("${dynamic.db}")
	private String dynaimc;

	@PostConstruct
	public void initialize() {
		if (dynaimc.equals("GCP")) {
			try {
				FileInputStream serviceAccount = new FileInputStream(
						"E:\\taliwind\\Stock API\\tailwind-traders-multicloud\\Backend\\serviceKey.json");
				FirebaseOptions options = FirebaseOptions.builder()
						.setCredentials(GoogleCredentials.fromStream(serviceAccount)).build();
				FirebaseApp.initializeApp(options);
			} catch (Exception e) {
				e.printStackTrace();
			}
		}

	}

}
