using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;
using TMPro;

public class DatabaseDemo : MonoBehaviour
{
    [SerializeField] private Player A, B;
    [SerializeField] private Button _aAttack, _bAttack;
    [SerializeField] private TextMeshProUGUI playerAText, playerBText;
    [SerializeField] private bool canAttack;
    async void Start()
    {   
        A = new Player();
        B = new Player();
        canAttack = true;
        _aAttack.onClick.AddListener(async () => 
        {
           if (!canAttack) { return; }        
           await Attack("Players/Player A", "Players/Player B");
           await SyncPlayers();
        });


        _bAttack.onClick.AddListener(async () => 
        {
           if (!canAttack) { return; }
           await Attack("Players/Player B", "Players/Player A");
           await SyncPlayers();
        });


        await SyncPlayers();
    }

    
     void Update()
    {
        playerAText.text  = "Health : " + A.Health.ToString() + " \n\n Damage : " + A.Attack.ToString(); 
        playerBText.text  = "Health : " + B.Health.ToString() + " \n\n Damage : " + B.Attack.ToString();
    }


    private async Task SyncPlayers()
    {
        A = await GameManager.GetInstance().fbManager.dbManager.GetNodeData("Players/Player A");
        B = await GameManager.GetInstance().fbManager.dbManager.GetNodeData("Players/Player B");
    }

    private async Task Attack(string attackerNode, string attackedNode)
    {
        //1. Get Both Player's Data
        Player attacker = await GameManager.GetInstance().fbManager.dbManager.GetNodeData(attackerNode);
        Player attacked = await GameManager.GetInstance().fbManager.dbManager.GetNodeData(attackedNode);

        attacked.Health -= attacker.Attack;

        if(attacked.Health < 0)
        {
            attacked.Health = -1;
        }

        await GameManager.GetInstance().fbManager.dbManager.UpdateNode(attackedNode, attacked);
        await GameManager.GetInstance().fbManager.dbManager.UpdateNode(attackerNode, attacker);

    }


}
