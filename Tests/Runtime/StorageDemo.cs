using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StorageDemo : MonoBehaviour
{


    [SerializeField] private InputField uploadPath, localPath;
    [SerializeField] private TextMeshProUGUI status;
    [SerializeField] private Button uploadBtn;
    public string str;
    void Start()
    {
        uploadBtn.onClick.AddListener(() => {

            Upload();

        });
    }


    void Update()
    {
        status.text = $"Status : {str}";
    }


    private async void Upload()
    {
       //str = (await GameManager.GetInstance().fbManager.sManager.UploadImage(localPath.text,uploadPath.text)) ? ("Upload Sucessful") : ("Error");
       str = (await GameManager.GetInstance().fbManager.sManager.UploadAudio(localPath.text,uploadPath.text)) ? ("Upload Sucessful") : ("Error");
    }
}
