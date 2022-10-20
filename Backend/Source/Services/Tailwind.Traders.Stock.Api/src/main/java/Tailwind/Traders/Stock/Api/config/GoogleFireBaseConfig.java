package Tailwind.Traders.Stock.Api.config;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.StringReader;
import java.net.URI;
import java.security.KeyFactory;
import java.security.NoSuchAlgorithmException;
import java.security.PrivateKey;
import java.security.spec.InvalidKeySpecException;
import java.security.spec.PKCS8EncodedKeySpec;
import java.util.Base64;
import java.util.Collection;
import java.util.Collections;

import javax.annotation.PostConstruct;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Configuration;

import com.google.auth.oauth2.GoogleCredentials;
import com.google.auth.oauth2.ServiceAccountCredentials;
import com.google.firebase.FirebaseApp;
import com.google.firebase.FirebaseOptions;

@Configuration
public class GoogleFireBaseConfig {

	@Value("${dynamic.db}")
	private String dynamic;

	@Value("${gcp.project_id}")
	private String gcpProject_id;
	@Value("${gcp.private_key_id}")
	private String gcpPrivate_key_id;
	@Value("${gcp.private_key}")
	private String gcpPrivate_key;
	@Value("${gcp.client_email}")
	private String gcpClient_email;
	@Value("${gcp.client_id}")
	private String gcpClient_id;
	@Value("${gcp.token_uri}")
	private String gcpToken_uri;

	@PostConstruct
	public void initialize() {
		if (dynamic.equals("GCP")) {
			try {

				Collection<String> scopes = Collections.singleton("https://www.googleapis.com/auth/cloud-language");
				GoogleCredentials sac = ServiceAccountCredentials.newBuilder()
						.setPrivateKey(getPrivateKey(gcpPrivate_key)).setPrivateKeyId(gcpPrivate_key_id)
						.setProjectId(gcpProject_id).setClientEmail(gcpClient_email).setScopes(scopes)
						.setTokenServerUri(new URI(gcpToken_uri)).setClientId(gcpClient_id).build();
				FirebaseOptions options = FirebaseOptions.builder().setCredentials(sac).build();

				FirebaseApp.initializeApp(options);
			} catch (Exception e) {
				e.printStackTrace();
			}
		}

	}
	/**
	 * getPrivateKey
	 * @param privateKey
	 * @return
	 * @throws NoSuchAlgorithmException
	 * @throws InvalidKeySpecException
	 * @throws IOException
	 */
	private PrivateKey getPrivateKey(String privateKey)
			throws NoSuchAlgorithmException, InvalidKeySpecException, IOException {
		StringBuilder pkcs8Lines = new StringBuilder();
		BufferedReader rdr = new BufferedReader(new StringReader(privateKey));
		String line;
		while ((line = rdr.readLine()) != null) {
			pkcs8Lines.append(line);
		}
		// Remove the "BEGIN" and "END" lines, as well as any whitespace
		String pkcs8Pem = pkcs8Lines.toString();
		pkcs8Pem = pkcs8Pem.replace("-----BEGIN PRIVATE KEY-----", "");
		pkcs8Pem = pkcs8Pem.replace("-----END PRIVATE KEY-----", "");
		pkcs8Pem = pkcs8Pem.replaceAll("\\s+", "");
		// Base64 decode the result
		byte[] pkcs8EncodedBytes = Base64.getDecoder().decode(pkcs8Pem); // Base64.decode(pkcs8Pem);
		// byte [] pkcs8EncodedBytes = Base64.
		// extract the private key
		PKCS8EncodedKeySpec keySpec = new PKCS8EncodedKeySpec(pkcs8EncodedBytes);
		KeyFactory kf = KeyFactory.getInstance("RSA");
		return kf.generatePrivate(keySpec);
	}

}
