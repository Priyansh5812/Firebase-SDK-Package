using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;




public class FirestoreDemo : MonoBehaviour
{
    [SerializeField] private Player A, B;
    [SerializeField] private TextMeshProUGUI _aStatus, _bStatus;
    [SerializeField] private Button _aAttack, _bAttack;
    [SerializeField] private bool canAttack;
    public async void Start()
    {
        A = new Player();
        B = new Player();

        await SyncPlayers();

        if (A.Health == 0 || B.Health == 0)
        {
            canAttack = false;
        }

        _aAttack.onClick.AddListener(async () =>
        {
            if (!canAttack) {return;}
            await Attack("Player A", "Player B");
        });
        _bAttack.onClick.AddListener(async () =>
        {
            if (!canAttack) { return; }
            await Attack("Player B", "Player A");
        });
    }


    public void Update()
    {   
        _aStatus.text =
            "Health : " + A.Health + "\n" +
            "Attack : " + A.Attack;
        _bStatus.text =
            "Health : " + B.Health + "\n" +
            "Attack : " + B.Attack;
    }

    private async Task Attack(string attacker, string attacked)
    {

        FirestoreResponseDocument Attacker_doc = await GameManager.GetInstance().fbManager.fsManager.GetCollectionFields("PlayerCollection" , attacker);
        FirestoreResponseDocument Attacked_doc = await GameManager.GetInstance().fbManager.fsManager.GetCollectionFields("PlayerCollection" , attacked);

        
        int newHealth = (int.Parse(Attacked_doc.fields.Health.stringValue)) - (int.Parse(Attacker_doc.fields.Attack.stringValue));
        if (newHealth <= 0) 
        {
            newHealth = 0;
            canAttack = false;
            //await GameManager.GetInstance().fbManager.fsManager.DeleteDocument("PlayerCollection", attacked);
        }

        Attacked_doc.fields.Health.stringValue = newHealth.ToString();

        switch (attacked)
        {
            case "Player A":
                A.Health = int.Parse(Attacked_doc.fields.Health.stringValue);
                break;

            case "Player B":
                B.Health = int.Parse(Attacked_doc.fields.Health.stringValue);
                break;

            default:
                return;
                
        }

        Debug.Log(await GameManager.GetInstance().fbManager.fsManager.UpdateField("PlayerCollection", attacked, "Health", newHealth.ToString()));

    }

    private async Task SyncPlayers()
    {
        FirestoreResponseDocument PlayerA_doc = await GameManager.GetInstance().fbManager.fsManager.GetCollectionFields("PlayerCollection", "Player A");
        FirestoreResponseDocument PlayerB_doc = await GameManager.GetInstance().fbManager.fsManager.GetCollectionFields("PlayerCollection", "Player B");

        A.Health = int.Parse(PlayerA_doc.fields.Health.stringValue);
        A.Attack = int.Parse(PlayerA_doc.fields.Attack.stringValue);
        
        B.Health = int.Parse(PlayerB_doc.fields.Health.stringValue);
        B.Attack = int.Parse(PlayerB_doc.fields.Attack.stringValue);

    }

}
