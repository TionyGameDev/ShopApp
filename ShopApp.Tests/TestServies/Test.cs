namespace ShopApp.Tests.TestServies;

public class Test
{
    public void MoveZeroes(int[] nums)
    {
        var left = 0;
        for (int i = 0; i < nums.Length; i++)
        {
            if (nums[left] != 0)
            {
                (nums[left], nums[i]) = (nums[i], nums[left]);
                left++;
            }
        }
        
    }
    public bool Duplicate(int[] nums)
    {
        var seen = new HashSet<int>();
        
        for (int i = 0; i < nums.Length; i++)
        {
            var complement = nums[i];
            if (!seen.Add(complement))
                return true;
        }

        return false;
    }
    
    public int FindMaximumSubarray(int[] nums)
    {
        int current = nums[0];
        int best = nums[0];

        for (int i = 1; i < nums.Length; i++)
        {
            current = Math.Max(current + nums[i], nums[i]);
            best = Math.Max(best, current);
        }

        return best;
    }
    public List<string> FizzBuzz(int n)
    {
        var list = new List<string>();
        for (int i = 1; i <= n; i++)
        {
            if (i % 5 == 0 && i % 3 == 0)
                list.Add($"FizzBuzz");
            else if (i % 3 == 0)
                list.Add($"Fizz");
            else if (i % 5 == 0 )
                list.Add($"Buzz");
            else
                list.Add(i.ToString());
        }
        return list;
    }
    
    public int[] TwoSum(int[] nums, int target)
    {
        var list = new List<int>();
        var dict = new Dictionary<int, int>();
        for (int i = 0; i < nums.Length; i++)
        {
            var complement = target - nums[i];
            if (dict.TryGetValue(complement, out var value))
            {
                list.Add(value);
                list.Add(i);
            }
            else
                dict.Add(nums[i], i);
        }
        
        return list.ToArray();
    }

    bool IsValid(string s)
    {
        var stack = new Stack<char>();
  
        foreach (var c in s)
        {
            if (c == '(' || c == '[' || c == '{')
            {
                stack.Push(c);
            }
            else
            {
                if (stack.Count == 0)
                    return false;
      
                var top = stack.Pop();
                if (c == ')' && top != '(') return false;
                if (c == ']' && top != '[') return false;
                if (c == '}' && top != '{') return false;
            }
        }
  
        return stack.Count == 0;
    }
    
    public bool IsPalindrome(string text)
    {
        var filtered = text
            .Where(char.IsLetterOrDigit)
            .Select(char.ToLower)
            .ToArray();
        
        int left = 0;
        int right = filtered.Length - 1;

        while (left < right)
        {
            if (filtered[left] != filtered[right])
                return false;
            left++;
            right--;
        }

        return true;

    } 
    
}