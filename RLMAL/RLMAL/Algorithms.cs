// ===============================
// AUTHOR       : Maaike Burghoorn, Wouter van 't Hof
// CREATE DATE  : November 2017
// COURSE       : Multi-agent systems - Utrecht University 2017/2018
// PURPOSE      : Reinforcement learning algorithms implementations
// SPECIAL NOTES: The specified assignments need to be implemented here.
// ===============================

using System;
using System.Linq;

namespace RLMAL
{
    static class Algorithms
    {
        /// TO DO: EXERCISE 1
        /// The agent action reward estimate update.
        /// RETURN      : The method should return the new reward estimate for the selected action.
        /// PARAMETERS
        /// agent       : the agent for which the reward needs to be updated
        /// alpha       : the learning parameter
        /// reward_t    : the reward the agent received at tick t from the chosen slot machine
        public static double updateScore(Agent agent, double alpha, double reward_t)
        {
            // return (agent.getRewards[agent.getMachineId] * (alpha-1) + reward_t) / alpha;
            return agent.getRewards[agent.getMachineId] * (1 - alpha) + reward_t * alpha;
        }

        /// TO DO: EXERCISE 2
        /// The optimistic initial values algorithm. 
        /// RETURN      : The method should return an action index (this represents the machine id).
        /// PARAMETERS
        /// agent       : the agent for which the action/slot machine is selected
        /// random      : this object can be used to generate random numbers
        public static int optimistic(Agent agent, Random random)
        {
            //Update the reward for the corresponding slot machine used in the previous round.

            //Find and return the best possible action based on the average rewards recieved per slot machine.
            int bestSlotIndex = 0;
            double bestReward = agent.getRewards[0];
            for (int i = 1; i < agent.getNrSlots; i++)
            {
                if (agent.getRewards[i] > bestReward)
                {
                    bestSlotIndex = i;
                    bestReward = agent.getRewards[i];
                }
                // Add a random chance to select a machine of equal value
                else if (agent.getRewards[i] == bestReward && random.NextDouble() > 0.5)
                    bestSlotIndex = i;
            }

            return bestSlotIndex;
        }

        /// TO DO: EXERCISE 3
        /// The egreedy algorithm.
        /// RETURN      : The method should return an action index (this represents the machine id).
        /// PARAMETERS
        /// agent       : the agent for which the action/slot machine is selected
        /// epsilon     : the random action selection parameter
        /// random      : this object can be used to generate random numbers
        public static int egreedy(double epsilon, Agent agent, Random random)
        {
            // There's a chance of epsilon to go to a random slot machine
            if (random.NextDouble() > epsilon)
                return random.Next(agent.getNrSlots);

            // Else perform the same thing as during optimistic search
            return optimistic(agent, random);
        }

        /// TO DO: EXERCISE 4
        /// The softmax action selection algorithm.
        /// RETURN      : The method should return an action index (this represents the machine id).
        /// PARAMETERS
        /// agent       : the agent for which the action/slot machine is selected
        /// tau         : temperature parameter in Gibbs/Boltzmann distribution
        /// random      : this object can be used to generate random numbers
        public static int softmax(double tau, Agent agent, Random random)
        {
            // Calculate the total reward (denominator of the Gibbs distribution)
            double totalRewards = 0;
            double[] p = new double[agent.getNrSlots];  // Chances per action

            for (int i = 0; i < agent.getNrSlots; i++)
            {
                p[i] = Math.Pow(Math.E, agent.getRewards[i] / tau);
                totalRewards += p[i];
            }

            // Calculate the chances per action to be chosen
            for (int i = 0; i < agent.getNrSlots; i++)
                p[i] /= totalRewards;

            // Get a random value from [0,1]
            double x = random.NextDouble();

            // The CDF of the values
            double cummulative = 0;

            // Once the value of x falls between the interval between the previous and the next value
            //   thus choosing the next action
            for (int i = 0; i < agent.getNrSlots; i++)
            {
                cummulative += p[i];
                if (x < cummulative)
                    return i;
            }

            return 0;
        }



    }
}
