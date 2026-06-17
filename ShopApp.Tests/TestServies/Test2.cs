// namespace ShopApp.Tests.TestServies;
//
// public class Test2
// {
//     public class TreeNode
//     {
//         public int Val;
//         public TreeNode Left;
//         public TreeNode Right;
//     }
//
//     public int MaxDepth(TreeNode root)
//     {
//         if (root == null) return 0;
//
//         int left = MaxDepth(root.Left);
//         int right = MaxDepth(root.Right);
//
//         return 1 + Math.Max(left, right);
//     }
//
//     public List<List<int>> ClimbingStairs(int n)
//     {
//         if (n < 1) 
//             return null!;
//         
//         var result = new List<List<int>>();
//         
//         
//         var index = 0;
//         for (int i = 1; i < n; i++)
//         {
//             if (i % 3 == 0)
//             {
//                 result[index].Add();
//             } 
//             index++;
//         }
//         
//     } 
// }