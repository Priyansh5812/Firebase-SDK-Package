using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
public class AuthDemo : MonoBehaviour
{
    [SerializeField] private TMP_InputField email, pass , code;
    [SerializeField] private TextMeshProUGUI log;
    [SerializeField] private Button signIn, signUp, GoogleCode , GoogleSignIn , PasteCodeBtn;
    [SerializeField] private string tokenId;
    void Start()
    {
        signIn.onClick.AddListener(async () => { await SignIn(); });
        signUp.onClick.AddListener(async () => { await SignUp(); });
        GoogleCode.onClick.AddListener(() => { GoogleGetCode(); });
        PasteCodeBtn.onClick.AddListener(() => { PasteCode(); });
        GoogleSignIn.onClick.AddListener(async () => { await GetAuthCodeViaGoogle(); });
    }

    // Update is called once per frame
    private async Task SignIn()
    {
       Auth_Response_Payload resp = await GameManager.GetInstance().fbManager.authManager.SignInUser(email.text , pass.text , true);
        if (resp.responseType)
        {
            log.text = "Login Sucess";
            this.tokenId = resp.idToken;
        }
        else
            log.text = "Login Failed";
    }

    private async Task SignUp() 
    {
      Auth_Response_Payload resp  = await GameManager.GetInstance().fbManager.authManager.SignUpUser(email.text , pass.text , true);
        if (resp.responseType)
        {
            log.text = "Login Sucess";
            this.tokenId = resp.idToken;
        }
        else
            log.text = "Login Failed";
        
    }

    private void GoogleGetCode()
    {
        GameManager.GetInstance().fbManager.authManager.GetAccessCode();
    }

    private void PasteCode()
    {
        code.text = GUIUtility.systemCopyBuffer;
    }

    private async Task GetAuthCodeViaGoogle()
    {
        if (string.IsNullOrEmpty(code.text))
        {
            Debug.LogWarning("Enter valid Code");
            return;
        }
       Auth_Response_Payload payload = await GameManager.GetInstance().fbManager.authManager.ExchangeAuthCodeWithIDToken(code.text);
        if (payload.responseType)
            log.text = "Login Success";
        else
            log.text = "Login Failed";
    }



}
