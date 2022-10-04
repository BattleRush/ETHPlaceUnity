from multiprocessing.spawn import old_main_modules
import re

# read file UnityBuild.framework.js
data = ""

with open('UnityBuild.framework.js', 'r') as f:
    data = f.read()

    # define regex
    regex = re.compile(r'(Runtime.dynCall\(")(vi{1,5})(",)([a-zA-Z.]+)(,\[)([a-zA-Z.]+)(,)?([a-zA-Z.]+)?(,)?([a-zA-Z.]+)?(,)?([a-zA-Z.]+)?(\])')

    # find all matches
    matches = regex.findall(data)

    # replace all matches
    for match in matches:
        
        #print(match)    
        new_string = "Module['dynCall_"

        # extract all groups
        vii = match[1]

        # add vii to new_string
        new_string += vii
        new_string += "']("
        new_string += match[3]

        if len(match) >= 6 and match[5] != "":
            new_string += ", "
            new_string += match[5]

        if len(match) >= 8 and match[7] != "":
            new_string += ", "
            new_string += match[7]

        if len(match) >= 10 and match[9] != "":
            new_string += ", "
            new_string += match[9]
        
        # concatinate
        old_string = ''.join(x for x in match)
        

        print(old_string)
        print(new_string)

        data = data.replace(old_string, new_string)

# write data back to file
with open('UnityBuild.framework.js', 'w') as f:
    f.write(data)