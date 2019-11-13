import re
import sys

path = sys.argv[1]
tag = sys.argv[2]

print(f'Search for version number in \'{tag}\'...')

result = re.match(r'refs/tags/v(\d+)\.(\d+)', tag)

if result:
    major = int(result[1])
    minor = int(result[2])

    print(f'Found version number {major}.{minor}')

    print(f'Patch version number in \'{path}\'...')

    with open(path) as f:
        content = f.read()

    content = content.replace('0.0.0.0', f'{major}.{minor}.0.0')

    with open(path, 'w') as f:
        f.write(content)

    print(f'done')