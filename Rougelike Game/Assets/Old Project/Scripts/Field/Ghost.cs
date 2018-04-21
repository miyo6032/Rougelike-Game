using UnityEngine;
using System.Collections;

public class Ghost : Enemy {

    public int lunge = 1;

    protected override void AttemptMove<T>(int xDir, int yDir)
    {

        float xDist = Mathf.Abs(target.position.x - transform.position.x);
        float yDist = Mathf.Abs(target.position.y - transform.position.y);

        if (altMove)
        {


            if ((xDist > 2 && Mathf.Abs(xDir) > 0) || (yDist > 2 && Mathf.Abs(yDir) > 0))
            {
                base.AttemptMove<T>(xDir * lunge, yDir * lunge);
            }
            else
            {
                base.AttemptMove<T>(xDir, yDir);
            }

            altMove = !altMove;
        }
        else
        {

            if ((xDist > 2 && Mathf.Abs(xDir) > 0) || (yDist > 2 && Mathf.Abs(yDir) > 0))
            {
                base.AttemptMove<T>(xDir * lunge, yDir * lunge);
            }
            else
            {
                base.AttemptMove<T>(xDir, yDir);
            }

            altMove = !altMove;
        }

    }

}
