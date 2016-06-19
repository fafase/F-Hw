using UnityEngine;
using System.Collections;

/// <summary>
/// Each product gets its own set of display, this is controlled from this class.
/// </summary>
public class TopProductController : MonoBehaviour 
{
	public ActionTypeController [] m_actionTypeController;

	/// <summary>
	/// When a new product is propagated, the product name is converted to a value (0 to 2).
	/// The action name is converted to a value (0 to 10)
	/// Both are transfered to update the appropriate product with the action.
	/// </summary>
	/// <param name="productName">Product name.</param>
	/// <param name="actionName">Action name.</param>
	public void SetNewEvent(int productName, int actionName)
	{
		if(productName >= this.m_actionTypeController.Length){ return; }
		ActionTypeController act = this.m_actionTypeController[productName];
		act.SetDailyGraph(actionName);
	}
}
